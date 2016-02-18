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
import org.apache.hadoop.fs.FileSystem;
import org.apache.hadoop.fs.Path;
import org.apache.hadoop.io.IntWritable;
import org.apache.hadoop.io.Text;
import org.apache.hadoop.mapreduce.Job;
import org.apache.hadoop.mapreduce.lib.input.SequenceFileInputFormat;
import org.apache.hadoop.mapreduce.lib.output.SequenceFileOutputFormat;
public class JobRunner {

	public static double T2 = 5;
	public static double T1 = 7;
	public static final Log LOG = LogFactory.getLog(JobRunner.class);

	public static void main(String[] args) throws IOException, InterruptedException, ClassNotFoundException {
		Path in = new Path("hadoop_proj/final");
		Path center = new Path("hadoop_proj/canopy/finalCenters");
		Path outCanopy = new Path("hadoop_proj/canopy");
		Path out = new Path("hadoop_proj/output");
		
		int k = Integer.parseInt(args[0]);
		
		boolean success = runCanopy(in,outCanopy,center, k);
		
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
		Path outKMeans = new Path("hadoop_proj/kmeans/depth_1");
		
		Job job = new Job(conf);
		job.setJobName("KMeans Clustering");

		job.setMapperClass(KMeansMapper.class);
		job.setReducerClass(KMeansReducer.class);
		job.setJarByClass(KMeansMapper.class);

		FileSystem fs = FileSystem.get(conf);

		if (fs.exists(outKMeans))
			fs.delete(outKMeans, true);
		
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
			
			dataPath = new Path("hadoop_proj/kmeans/depth_" + (iteration - 1) + "/");
			outKMeans = new Path("hadoop_proj/kmeans/depth_" + iteration);

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

		LOG.info("KMeans Iteration: " + (iteration - 1));
		
		Path result = new Path("hadoop_proj/kmeans/depth_" + (iteration - 1) +"/part-r-00000");
        try {
            BufferedReader br=new BufferedReader(new InputStreamReader(fs.open(result)));
            String line;
            line=br.readLine();
    		Map<String, List<String>> clustersMap = new HashMap<String, List<String>>();
    		List<String> l ;
    		
            while (line != null){
                    String[] seperatedVals1 = line.split("\t");
                    l = clustersMap.get(seperatedVals1[2]); 
                    if (l != null){
                        l.add(seperatedVals1[3]);
                    }
                    else{
                    	l = new ArrayList<String>();
                        l.add(seperatedVals1[3]);
                    	clustersMap.put(seperatedVals1[2], l);
                    }
                    line=br.readLine();
            }

    		BufferedWriter out2 = new BufferedWriter(new FileWriter("output"));

            for(Entry<String, List<String>> entry : clustersMap.entrySet()) {
            	
            	for (String stock : entry.getValue()) {
            		out2.write(stock + " ");
				}
            	out2.newLine();
            }
           
            out2.close();
            br.close();
            
	    } catch(Exception e){
        	LOG.info("ERROR!:  "+ e.getMessage()+ " " + e.getCause() + "\n" + e.getStackTrace());	
	    }
		
		return success;
	}
	
	public static boolean runCanopy(Path in, Path out, Path centers ,int k) throws IOException, InterruptedException, ClassNotFoundException {
		
		Configuration conf = new Configuration();  
		conf.set("k", Integer.toString(k));
		conf.set("centroid.path", centers.toString());
		Job job = new Job(conf);
		
	    job.setJarByClass(JobRunner.class);
	    
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
}
