package solution;

import java.io.BufferedReader;
import java.io.BufferedWriter;
import java.io.FileWriter;
import java.io.IOException;
import java.io.InputStreamReader;
import java.util.ArrayList;
import java.util.HashMap;
import java.util.List;
import java.util.Map;
import java.util.Map.Entry;

import org.apache.commons.logging.Log;
import org.apache.commons.logging.LogFactory;
import org.apache.hadoop.conf.Configuration;
import org.apache.hadoop.fs.FileStatus;
import org.apache.hadoop.fs.FileSystem;
import org.apache.hadoop.fs.Path;
import org.apache.hadoop.io.IntWritable;
import org.apache.hadoop.io.LongWritable;
import org.apache.hadoop.io.SequenceFile;
import org.apache.hadoop.io.Text;
import org.apache.hadoop.mapreduce.Job;
import org.apache.hadoop.mapreduce.lib.input.SequenceFileInputFormat;
import org.apache.hadoop.mapreduce.lib.output.SequenceFileOutputFormat;
public class CanopyJob {

	public static double T2 = 5;
	public static double T1 = 7;
	public static final Log LOG = LogFactory.getLog(CanopyJob.class);

	public static void main(String[] args) throws IOException, InterruptedException, ClassNotFoundException {
		Path in = new Path("anna/final");
		Path center = new Path("anna/canopy/part-r-00000");
		Path outCanopy = new Path("anna/canopy");
		Path out = new Path("anna/output");
		

		Configuration conf = new Configuration();  
		FileSystem fs = FileSystem.get(conf);

		
/*		if (fs.exists(out))
			fs.delete(out, true);*/
		
		int k = 3;//Integer.parseInt(args[0]);
		
		boolean success = runCanopy(in,outCanopy);
		
		if (success) {
			success = runKMeans(in, out, center, k);
		}
		
	    System.exit(success ? 0 : 1);
	}
	
	public static boolean runKMeans(Path dataPath, Path outPath, Path centers,int k) throws IOException, InterruptedException, ClassNotFoundException {
		
		int iteration = 1;
		Configuration conf = new Configuration();
		conf.set("num.iteration", iteration + "");
		conf.set("centroid.path", centers.toString());
		Path outKMeans = new Path("anna/kmeans/depth_1");
		Path seq_input = new Path("anna/seq");
		
		Job job = new Job(conf);
		job.setJobName("KMeans Clustering");

		job.setMapperClass(KMeansMapper.class);
		job.setReducerClass(KMeansReducer.class);
		job.setJarByClass(KMeansMapper.class);

		FileSystem fs = FileSystem.get(conf);

		if (fs.exists(outKMeans))
			fs.delete(outKMeans, true);
		
		if (fs.exists(seq_input))
			fs.delete(seq_input, true);
/*		
		ArrayList<Vector> stockList = createVectorsFromFile(dataPath.toString());
		LOG.error("STOCK " + stockList.size());
		final SequenceFile.Writer dataWriter = SequenceFile.createWriter(fs, conf, seq_input, ClusterCenter.class, Vector.class);

		// Write the stocks
		for (int i = 0; i < stockList.size(); i++) {
			dataWriter.append(new ClusterCenter(new Vector(0, 0)), stockList.get(i));
		}
		
		dataWriter.close();*/
		
		SequenceFileInputFormat.setInputPaths(job, dataPath);
		SequenceFileOutputFormat.setOutputPath(job, outKMeans);
		
		job.setOutputKeyClass(Text.class);
		job.setOutputValueClass(Text.class);
		job.setMapOutputKeyClass(ClusterCenter.class);
		job.setMapOutputValueClass(Vector.class);
		
		boolean success = job.waitForCompletion(true);

		long counter = job.getCounters().findCounter(KMeansReducer.Counter.CONVERGED).getValue();
		
		iteration++;
		
		while (counter > 0 && success) {
			conf = new Configuration();
			conf.set("centroid.path", centers.toString());
			conf.set("num.iteration", iteration + "");
			job = new Job(conf);
			job.setJobName("KMeans Clustering " + iteration);
			job.setMapperClass(KMeansMapper.class);
			job.setReducerClass(KMeansReducer.class);
			job.setJarByClass(KMeansMapper.class);
			
			dataPath = new Path("anna/kmeans/depth_" + (iteration - 1) + "/");
			outKMeans = new Path("anna/kmeans/depth_" + iteration);

			SequenceFileInputFormat.setInputPaths(job, dataPath);;
			
			if (fs.exists(outKMeans))
				fs.delete(outKMeans, true);

			SequenceFileOutputFormat.setOutputPath(job,outKMeans);
			job.setOutputKeyClass(Text.class);
			job.setOutputValueClass(Text.class);
			job.setMapOutputKeyClass(ClusterCenter.class);
			job.setMapOutputValueClass(Vector.class);

			success = job.waitForCompletion(true);
			iteration++;
			counter = job.getCounters().findCounter(KMeansReducer.Counter.CONVERGED).getValue();
		}

		LOG.info("COUNTERRRR: " + iteration);
		Path result = new Path("anna/kmeans/depth_" + (iteration - 1) +"/part-r-00000");
        try{
            BufferedReader br=new BufferedReader(new InputStreamReader(fs.open(result)));
            String line;
            line=br.readLine();
    		String str = "";
    		Map<String, List<String>> clustersMap = new HashMap<String, List<String>>();
    		List<String> l ;
    		
            while (line != null){
                    String[] seperatedVals1 = line.split("\t");
                    l = clustersMap.get(seperatedVals1[2]); 
                    if (l != null){
                    	LOG.info("ADD VALUE:  " + seperatedVals1[3]);	
                        l.add(seperatedVals1[3]);
                    }
                    else{
                    	l = new ArrayList<String>();
                    	LOG.info("NEW VALUE:  " + seperatedVals1[3]);	
                        l.add(seperatedVals1[3]);
                    	clustersMap.put(seperatedVals1[2], l);
                    	LOG.info("CREATE " + seperatedVals1[2]);	

                    }
                    line=br.readLine();


                  /*  
                    br2=new BufferedReader(new InputStreamReader(fs.open(result)));
                    line2 = br2.readLine();
                    while(line2 != null){
                        String[] seperatedVals2 = line2.split("\t");
                        if(seperatedVals2[2].equals(seperatedVals1[2])){
                        	LOG.info("VALUEE:  " + seperatedVals2[3]);	
                        	str += seperatedVals2[3] + " ";
                        }
                        str += "\n";
                    }
            		br2.close();
                    out2.write(str);*/
            }

        	LOG.info("PRINT:  "+ clustersMap.size());	
    		BufferedWriter out2 = new BufferedWriter(new FileWriter("output"));

            for(Entry<String, List<String>> entry : clustersMap.entrySet()) {
            	
            	LOG.info("PRINT:  "+ entry.getKey() + ", " + entry.getValue().size());	
            	for (String stock : entry.getValue()) {
                	//str += stock + " ";
            		out2.write(stock + " ");
				}
                //str += "\n";
            	out2.newLine();
            }
           
    		
            //out2.write(str);
            out2.close();
            br.close();
            
            
	    }catch(Exception e){
        	LOG.info("ERROR!:  "+ e.getMessage()+ " " + e.getCause() + "\n" + e.getStackTrace());	

	    }
        /*
		SequenceFile.Reader reader1 = new SequenceFile.Reader(fs, result, conf);
		SequenceFile.Reader reader2 = new SequenceFile.Reader(fs, result, conf);
		ClusterCenter key = new ClusterCenter();
		ClusterCenter key2 = new ClusterCenter();
		Vector v = new Vector();
		BufferedWriter out2 = new BufferedWriter(new FileWriter("output"));

		String str = "";
		
		for(int i=0;i<k;i++){
			String is = Integer.toString(i);
			reader2 = new SequenceFile.Reader(fs, result, conf);
			while(reader2.next(key2,v)){
				if(is.equals(key2.toString())){
					str += v + " ";
				}
			}
			str += "\n";
			reader2.close();
		}*/
		/*while(reader1.next(key2, v)){
			str += key2 + " " + v + "\n";
		}*/
		//out2.write(str);

		//reader1.close();
		//out2.close();
		
		return success;
	}
	
	public static boolean runCanopy(Path in, Path out) throws IOException, InterruptedException, ClassNotFoundException {
		Job job = new Job();
		Configuration conf = new Configuration();  
		
	    job.setJarByClass(CanopyJob.class);
	    
	    job.setJobName("Canopy Clustering");
	    
		FileSystem fs = FileSystem.get(conf);
		if (fs.exists(out))
			fs.delete(out, true);
		
		SequenceFileInputFormat.setInputPaths(job, in);
		SequenceFileOutputFormat.setOutputPath(job, out);
		
	    job.setMapperClass(CanopyMapper.class);
	    job.setReducerClass(CanopyReducer.class);

	    job.setMapOutputKeyClass(ClusterCenter.class);
	    job.setMapOutputValueClass(Vector.class);
	    
		job.setOutputFormatClass(SequenceFileOutputFormat.class);
		job.setOutputKeyClass(ClusterCenter.class);
		job.setOutputValueClass(IntWritable.class);
		
	    job.setNumReduceTasks(1);
	    
	    return job.waitForCompletion(true);
	}
	
	public static ArrayList<Vector> createVectorsFromFile(String path)
			throws IOException {
		ArrayList<Vector> result = new ArrayList<Vector>();
		BufferedReader br = null;
        try {
            FileSystem fs = FileSystem.get(new Configuration());
            FileStatus[] status = fs.listStatus(new Path("http://localhost:8888/user/training/" + path));
            for (int i=0;i<status.length;i++){
                    br=new BufferedReader(new InputStreamReader(fs.open(status[i].getPath())));
                    String line;
                    line=br.readLine();
                    while (line != null){
                            System.out.println(line);
                            line= br.readLine();
                			Vector currVector = new Vector(new Text(line));
                			result.add(currVector);
                    }
            }
	    } catch(Exception e){
	            System.out.println("File not found");
	    } finally {
	    	if (br != null) {
	    		br.close();
	    	}
	    }
        
		/*while (in.ready()) {
			String stockLine = in.readLine();
			String[] seperatedVals = stockLine.split(" ");

			String symbol = seperatedVals[0];

			double[] OnlyValsDoubles = new double[seperatedVals.length];
			for (int i = 1; i < seperatedVals.length; i++) {
				try {

					OnlyValsDoubles[i] = Double.parseDouble(seperatedVals[i]);
				} catch (NumberFormatException nfe) {
					// problem - I get a number with two dots
					OnlyValsDoubles[i] = 0.5;
				} finally {
					if (OnlyValsDoubles[i] == 0) {
						OnlyValsDoubles[i] = 0.5;
					}
				}

			}

			Vector currVector = new Vector();
			currVector.setStockName(symbol);
			currVector.setVector(OnlyValsDoubles);
			result.add(currVector);
		}

		in.close();*/
		return result;
	}
}
